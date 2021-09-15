import torch
import yaml
from SphereDetector.run_configuration import RunConfiguration
from train import train_model, validate_model


if __name__ == "__main__":
    if torch.cuda.is_available():
        device = torch.device('cuda')
    else:
        device = torch.device('cpu')

    with open('config.yaml', 'r') as config_file:
        config_dict = yaml.safe_load(config_file)
        config = RunConfiguration(config_dict)

    # train_model(config=config)
    validate_model(config=config)
