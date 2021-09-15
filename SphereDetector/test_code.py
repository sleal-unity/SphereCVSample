import torch
import torch.nn as nn
import torch.nn.functional as F
import torchvision
import torchvision.transforms as transforms
import torch.optim as optim
import matplotlib.pyplot as plt
import matplotlib.patches as patches
import numpy as np
from PIL import Image
from datasetinsights.datasets.unity_perception import Captures


DATA_ROOT = 'data/RedBall/synthetic/Test1'


class BoundingBox:
    def __init__(self, x, y, width, height):
        self.x = x
        self.y = y
        self.width = width
        self.height = height

    @property
    def position(self):
        return self.x, self.y


def get_bbox(df, index):
    bb = df['annotations'].iloc[index][0]['values'][0]
    return BoundingBox(bb['x'], bb['y'], bb['width'], bb['height'])


def tutorial():
    if torch.cuda.is_available():
        device = torch.device('cuda')
    else:
        device = torch.device('cpu')

    captures = Captures(data_root=DATA_ROOT)
    df = captures.captures
    bb = get_bbox(df, 0)
    filename = df['filename'].iloc[0]
    image = Image.open(DATA_ROOT + '/' + filename).convert('RGB')

    image_tensor = torch.tensor(np.array(image), device=device).permute(2, 0, 1)

    # Create figure and axes
    fig, ax = plt.subplots()

    # Display the image
    ax.imshow(image_tensor.cpu())

    # Create a Rectangle patch
    rect = patches.Rectangle(bb.position, bb.width, bb.height, linewidth=1, edgecolor='r', facecolor='none')

    # Add the patch to the Axes
    ax.add_patch(rect)

    plt.show()

    print(image_tensor.shape)


if __name__ == '__main__':
    tutorial()
