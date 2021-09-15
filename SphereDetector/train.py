import time
import torch
from SphereDetector.model import SphereDetectionNetwork
from sphere_dataset import SphereDataset, CocoSphereDataset


def train_model(*, config):
    # train_dataset = SphereDataset(
    #     data_root=config.train.data_root,
    #     input_res=config.dataset.input_resolution,
    #     source_res=config.dataset.source_resolution,
    #     device=config.device
    # )
    train_dataset = CocoSphereDataset(
        data_root=config.coco.train.data_root,
        input_res=config.dataset.input_resolution,
        device=config.device
    )
    train_loader = torch.utils.data.DataLoader(
        train_dataset,
        batch_size=config.train.batch_size,
        num_workers=0,
        drop_last=True,
        shuffle=True
    )

    # val_dataset = SphereDataset(
    #     data_root=config.val.data_root,
    #     input_res=config.dataset.input_resolution,
    #     source_res=config.dataset.source_resolution,
    #     device=config.device
    # )
    val_dataset = CocoSphereDataset(
        data_root=config.coco.val.data_root,
        input_res=config.dataset.input_resolution,
        device=config.device
    )
    val_loader = torch.utils.data.DataLoader(
        val_dataset,
        batch_size=config.val.batch_size,
        num_workers=0,
        drop_last=True,
        shuffle=True
    )

    train_loop(
        config=config,
        train_dataloader=train_loader,
        val_dataloader=val_loader,
    )


def validate_model(*, config):
    val_dataset = SphereDataset(
        data_root=config.test.data_root,
        input_res=config.dataset.input_resolution,
        source_res=config.dataset.source_resolution,
        device=config.device
    )
    # val_dataset = CocoSphereDataset(
    #     data_root=config.test.data_root,
    #     input_res=config.dataset.input_resolution,
    #     device=config.device
    # )
    val_loader = torch.utils.data.DataLoader(
        val_dataset,
        batch_size=config.test.batch_size,
        num_workers=0,
        drop_last=True,
        shuffle=True
    )

    model = torch.load(config.test.validation_model_path).to(config.device)
    _evaluate_one_epoch(
        model=model,
        data_loader=val_loader,
        epoch=1,
    )


def train_loop(*, config, train_dataloader, val_dataloader):
    starting_epoch = config.train.start_from_epoch
    if starting_epoch <= 1:
        model = SphereDetectionNetwork().to(config.device)
        starting_epoch = 1
    else:
        print(f'Continuing from epoch {starting_epoch}')
        model = torch.load(f'{config.checkpoint.models_directory}/{starting_epoch}.pt').to(config.device)
        starting_epoch += 1

    params = [p for p in model.parameters() if p.requires_grad]

    optimizer = torch.optim.Adam(
        params,
        betas=(config.adam_optimizer.beta_1, config.adam_optimizer.beta_2),
        lr=config.adam_optimizer.learning_rate,
    )

    epochs_count = config.train.epochs
    for epoch in range(starting_epoch, epochs_count + 1):
        print(f"Training Epoch[{epoch}/{epochs_count}]")
        _train_one_epoch(
            model=model,
            optimizer=optimizer,
            data_loader=train_dataloader,
            epoch=epoch,
        )

        if epoch % config.checkpoint.model_save_frequency == 0:
            filepath = f'models/{epoch}.pt'
            print(f'Saving model to {filepath}')
            torch.save(model, filepath)

        if epoch % config.val.eval_freq == 0:
            _evaluate_one_epoch(
                model=model,
                data_loader=val_dataloader,
                epoch=epoch,
            )


def _train_one_epoch(*, model, optimizer, data_loader, epoch):
    model.train()
    optimizer.zero_grad()

    start = time.time()
    metric_bbox = evaluation_over_batch(
        model=model,
        data_loader=data_loader,
        is_training=True,
        optimizer=optimizer,
        criterion_bbox=torch.nn.MSELoss(),
    )

    print(f'training metric bbox: {metric_bbox} at epoch {epoch}')
    print(f'time to execute epoch: {time.time() - start} seconds')


def _evaluate_one_epoch(*, model, data_loader, epoch):
    start = time.time()
    with torch.no_grad():
        metric_bbox = evaluation_over_batch(
            model=model,
            data_loader=data_loader,
            is_training=False,
            criterion_bbox=torch.nn.MSELoss(),
        )
        print(f'validation metric bbox: {metric_bbox} at epoch {epoch}')
        print(f'time to validate: {time.time() - start} seconds')


def evaluation_over_batch(
        *,
        model,
        data_loader,
        is_training=True,
        optimizer=None,
        criterion_bbox=None,
):
    metric_bbox = 0
    batch_index = 0

    for images, bbox_labels in data_loader:
        batch_index += 1
        if batch_index % (len(data_loader) / 5) == 0:
            print(f'\tbatch index [{batch_index}/{len(data_loader)}]')
        output_bbox = model(images)
        metric_bbox += bbox_average_mean_square_error(output_bbox, bbox_labels)

        if is_training:
            loss_bbox = criterion_bbox(output_bbox, bbox_labels)
            loss_bbox.backward()
            optimizer.step()
            optimizer.zero_grad()

    return metric_bbox / len(data_loader)


def bbox_average_mean_square_error(output, target):
    if target.shape != output.shape:
        raise ValueError(
            f"The shapes of real data {target.shape} and predicted data {output.shape} should be the same.")

    square_diffs = (target - output) ** 2
    norms = torch.mean(square_diffs, 1)
    bbox_metric = torch.mean(norms, axis=-1).item()

    return bbox_metric
