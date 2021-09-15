import torch
import torchvision


class SphereDetectionNetwork(torch.nn.Module):
    def __init__(self):
        super(SphereDetectionNetwork, self).__init__()
        self.model_backbone = torchvision.models.vgg16(pretrained=True)

        # remove the original classifier
        self.model_backbone.classifier = torch.nn.Identity()

        self.bbox_block = torch.nn.Sequential(
            torch.nn.Linear(25088, 256),
            torch.nn.ReLU(inplace=True),
            torch.nn.Linear(256, 64),
            torch.nn.ReLU(inplace=True),
            torch.nn.Linear(64, 4),
        )

    def forward(self, x):
        x = self.model_backbone(x)
        output_bbox = self.bbox_block(x)
        return output_bbox
