import json

import cv2
from PIL import Image
from datasetinsights.datasets.unity_perception import Captures
from torch.utils.data import Dataset
import logging
import torch
import torchvision
import numpy as np
import matplotlib.pyplot as plt

logger = logging.getLogger(__name__)


class SphereDataset(Dataset):

    def __init__(self, *, data_root, input_res=224, source_res=512, device='cpu'):
        self.device = device
        self.input_res = input_res
        self.source_res = source_res
        self.data_root = data_root
        self.captures = Captures(data_root=data_root)
        self.df = self.captures.captures
        self.image_transform = self._get_image_transform()

    def __len__(self):
        return self.df.shape[0]

    def __getitem__(self, index):
        return self._get_image_tensor(index), self._get_norm_bbox_tensor(index)

    def _get_bbox(self, index):
        bb = self.df['annotations'].iloc[index][0]['values'][0]
        return (np.array([bb['x'], bb['y'], bb['width'], bb['height']]) / self.source_res) * self.input_res

    def _get_bbox_tensor(self, index):
        return torch.tensor(self._get_bbox(index), device=self.device, dtype=torch.float32)

    def _get_norm_bbox_tensor(self, index):
        return self._get_bbox_tensor(index) / self.source_res

    def _get_image_tensor(self, index):
        filename = self.df['filename'].iloc[index]
        image = Image.open(f'{self.data_root}/{filename}').convert('RGB')
        image_tensor = self.image_transform(image).to(self.device)
        return image_tensor

    def _get_image_transform(self):
        transform = torchvision.transforms.Compose(
            [
                torchvision.transforms.Resize((self.input_res, self.input_res)),
                torchvision.transforms.ToTensor(),
            ]
        )
        return transform


class VideoSphereDataset(Dataset):

    def __init__(self, *, video_file_path, input_res=224, source_width=1920, source_height=1080, device='cpu'):
        self.video_file_path = video_file_path
        self.frames = torchvision.io.video.read_video(filename=video_file_path, pts_unit='sec')[0].permute(0, 3, 1, 2)
        self.device = device
        self.input_res = input_res
        self.source_width = source_width
        self.source_height = source_height
        self.image_transform = self._get_image_transform()

    def __len__(self):
        return len(self.frames)

    def __getitem__(self, index):
        return self._get_image_tensor(index)

    def _get_image_tensor(self, index):
        image = self.frames[index].to(self.device)
        return self.image_transform(image).float() / 255

    def _get_image_transform(self):
        transform = torchvision.transforms.Compose(
            [
                torchvision.transforms.Pad((0, 420)),
                torchvision.transforms.Resize((self.input_res, self.input_res))
            ]
        )
        return transform


class WebcamSphereDataset(Dataset):

    def __init__(self, *, input_res=224, device='cpu'):
        self.cam = cv2.VideoCapture(0)
        self.cam_height = self.cam.get(4)
        self.device = device
        self.input_res = input_res
        self.image_transform = self._get_image_transform()

    def __len__(self):
        return 1000

    def __getitem__(self, index):
        return self._get_image_tensor()

    def _get_image_tensor(self):
        ret = False
        while not ret:
            ret, frame = self.cam.read()
        return self.image_transform(frame).to(self.device)[[2, 1, 0], :, :]

    def _get_image_transform(self):
        transform = torchvision.transforms.Compose(
            [
                torchvision.transforms.ToTensor(),
                torchvision.transforms.CenterCrop(self.cam_height),
                torchvision.transforms.Resize((self.input_res, self.input_res))
            ]
        )
        return transform


class CocoSphereDataset(Dataset):

    def __init__(self, *, input_res=224, data_root='data/real/phone', device='cpu'):
        self.device = device
        self.input_res = input_res
        self.data_root = data_root
        self._get_annotations()
        self.image_transform = self._get_image_transform()

    def __len__(self):
        return len(self.annotations['annotations'])

    def __getitem__(self, index):
        bbox = self.annotations['annotations'][index]['bbox']
        bbox_array = self._transform_bbox(np.array([bbox[0], bbox[1], bbox[2], bbox[3]]))
        label = torch.tensor(bbox_array, dtype=torch.float32, device=self.device)
        return self._get_image_tensor(index), label

    def _get_annotations(self):
        with open(f'{self.data_root}/annotations/instances_default.json') as file:
            self.annotations = json.load(file)
        self.source_width = int(self.annotations['images'][0]['width'])
        self.source_height = int(self.annotations['images'][0]['height'])

    def _get_image_tensor(self, index):
        file_name = self.annotations['images'][index]['file_name']
        image = Image.open(f'{self.data_root}/images/{file_name}').convert('RGB')
        return self.image_transform(image).to(self.device)

    def _get_image_transform(self):
        transformations = []
        if self.source_height < self.source_width:
            padding = int((self.source_width - self.source_height) / 2)
            transformations.append(torchvision.transforms.Pad((0, padding)))
        transformations += [
            torchvision.transforms.Resize((self.input_res, self.input_res)),
            torchvision.transforms.ToTensor()
        ]
        transform = torchvision.transforms.Compose(transformations)
        return transform

    def _transform_bbox(self, bbox_array):
        if self.source_height < self.source_width:
            padding = (self.source_width - self.source_height) / 2
            bbox_array[1] += padding
        bbox_array = bbox_array / self.source_width
        return bbox_array
