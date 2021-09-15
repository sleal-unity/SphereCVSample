import cv2
import torch
import yaml
import matplotlib.pyplot as plt
from matplotlib import patches
from SphereDetector.run_configuration import RunConfiguration
from SphereDetector.sphere_dataset import SphereDataset, VideoSphereDataset, WebcamSphereDataset, CocoSphereDataset

image_index = 0
model = None
dataset = None
config = None
fig, ax = plt.subplots()


def draw_bbox(image, bb, label=None):
    global fig, ax
    ax.clear()
    ax.imshow(image.permute(1, 2, 0).cpu())
    if label is not None:
        rect = patches.Rectangle((label[0], label[1]), label[2], label[3], linewidth=4, edgecolor='g', facecolor='none')
        ax.add_patch(rect)
    rect = patches.Rectangle((bb[0], bb[1]), bb[2], bb[3], linewidth=2, edgecolor='r', facecolor='none')
    ax.add_patch(rect)


def inspect_dataset(event):
    global image_index
    if image_index < len(dataset):
        data = dataset[image_index]
        label = None
        if type(data) is tuple:
            image, label = data
            label = label * config.dataset.input_resolution
            label = label.cpu()
        else:
            image = data
        output_bbox = model(image.unsqueeze(0)).squeeze(0) * config.dataset.input_resolution
        output_bbox = output_bbox.detach().cpu()
        draw_bbox(image, output_bbox, label=label)
        plt.draw()
    else:
        plt.close()
    image_index += 1


def webcam_inspection():
    while True:
        image = dataset[0]
        output_bbox = model(image.unsqueeze(0)).squeeze(0) * config.dataset.input_resolution
        output_bbox = output_bbox.detach().cpu()
        draw_bbox(image, output_bbox)
        plt.draw()
        plt.pause(0.1)


if __name__ == "__main__":
    if torch.cuda.is_available():
        device = torch.device('cuda')
    else:
        device = torch.device('cpu')

    with open('config.yaml', 'r') as config_file:
        config_dict = yaml.safe_load(config_file)
        config = RunConfiguration(config_dict)

    with torch.no_grad():
        model = torch.load(config.test.validation_model_path).to(device)
        model.eval()
        for param in model.model_backbone.features.parameters():
            param.requires_grad = False

        # dataset = SphereDataset(
        #     data_root=config.val.data_root,
        #     input_res=config.dataset.input_resolution,
        #     source_res=config.dataset.source_resolution,
        #     device=config.device
        # )
        # dataset = VideoSphereDataset(
        #     video_file_path=config.real.data_root,
        #     device=config.device
        # )
        # dataset = CocoSphereDataset(
        #     data_root=config.coco.data_root,
        #     input_res=config.dataset.input_resolution,
        #     device=config.device
        # )
        # dataset = CocoSphereDataset(
        #     data_root=config.train.data_root,
        #     input_res=config.dataset.input_resolution,
        #     device=config.device
        # )
        #
        # plt.connect('key_press_event', inspect_dataset)
        # plt.show()

        dataset = WebcamSphereDataset(device=config.device)
        webcam_inspection()


