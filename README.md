# HSIFlow Lab

HSIFlow Lab is a C#/.NET project for working with hyperspectral imaging (HSI) data, with a focus on making hyperspectral analysis and machine-learning workflows more accessible.

The project brings together several libraries and an experimental WPF application:

* **HSIDataKit** — Core hyperspectral data models and I/O.
* **SpectrumKit** — Spectral visualization and preprocessing tools.
* **MaskingKit** — Hyperspectral masking and spectral similarity algorithms.
* **HSIModelKit** — Models, prediction infrastructure, and machine-learning integration.
* **HSIApp** — A desktop WPF application for viewing and analyzing hyperspectral images.

The long-term goal is to provide a practical desktop environment where users can load hyperspectral images, explore and preprocess spectra, create masks, visualize results, and eventually apply trained machine-learning models to selected regions of hyperspectral data.

> **Status:** Active development. The project is currently focused on building and testing the underlying libraries.

## Technologies

* C# / .NET
* WPF
* Hyperspectral imaging
* Spectral preprocessing and analysis
* Python integration for machine-learning models

## Project Structure

```text
HSIFlow Lab
├── HSIDataKit
├── SpectrumKit
├── MaskingKit
├── HSIModelKit
└── HSIApp
```

This repository is primarily a development and research project, and APIs and architecture may change as the project evolves.
