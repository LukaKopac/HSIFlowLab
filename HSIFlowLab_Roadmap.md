# HSI Desktop Application Roadmap

**Project name:** HSIFlow Lab

**Author:** Luka Kopač

**Status:** In Development

---

# Vision

Create a desktop application that allows researchers to analyze hyperspectral images and apply trained machine learning models without writing code.

The application should eventually provide a complete workflow within an intuitive graphical interface suitable for everyday use.

## Long-Term Workflow

```
Load Cubes
↓
Inspect
↓
Preprocess
↓
Mask
↓
Predict
↓
Analyze
↓
Export
```

---

# Version Roadmap

---

# Version 0.1 — Simple Viewer

### Viewer

* Close cube
* Recent files (optional)
* Previous / Next buttons
* Display current band number / wavelength

---

### Metadata

* filename
* width
* height
* bands
* datatype
* interleave
* byte order
* wavelength range

---

### Nice to Have

* Image title
* Status bar
* Loading indicator
* Keyboard shortcuts

---

# Version 0.2 — Better Viewer

Goal

Turn the viewer into a proper inspection tool.

## Features

Pixel coordinates

Crosshair

Image histogram

Image statistics

Current pixel value / intensity

Colormap selection

Contrast stretching

Percentile normalization

Min-max normalization

---

# Version 0.3 — Spectral Exploration

Goal

Allow users to inspect spectra.

## Features

Interactive spectrum plot

Hover mode

Click mode

ROI mode

Rectangle selection

Mean spectrum

Multiple spectra

Spectrum manager

Spectrum legend

Show / hide spectra

Export spectrum

Import spectrum

Save spectrum

Compare spectra

---

## Completion Criteria

The application becomes useful for exploratory analysis.

---

# Version 0.4 — Project Explorer

Goal

Support multiple datasets.

## Features

Project panel

Multiple cubes

Rename entries

Remove cubes

Current selection

Project save

Project load

Drag and drop cubes

Batch loading

Project notes

---

## Completion Criteria

Multiple cubes can be managed simultaneously.

---

# Version 0.5 — Preprocessing

Goal

Prepare data before prediction.

## Features

Normalization

SNV

MSC

Savitzky-Golay

Band selection

Cropping

ROI extraction

Block averaging

Preview preprocessing

Undo

Reset preprocessing

---

## Completion Criteria

All preprocessing can be performed from the GUI.

---

# Version 0.6 — Masking

Goal

Create and manage masks.

## Features

Threshold masking

KMeans masking

Manual masking

Brush tool

Polygon ROI

Load mask

Save mask

Invert mask

Overlay transparency

Mask statistics

Mask manager

Apply mask

Remove mask

Preview mask

---

## Completion Criteria

Users can isolate pixels before prediction.

---

# Version 0.7 — Prediction

Goal

Run trained models.

## Features

Load model

Model metadata

Predict current cube

Predict batch

Prediction progress

Prediction overlay

Confidence map

Prediction legend

Prediction statistics

Export prediction

---

## Completion Criteria

A researcher can use a trained model without Python.

---

# Version 0.8 — Results

Goal

Analyze predictions.

## Features

Pixel counts

Area percentages

Summary statistics

Confusion matrix

Class distributions

Interactive plots

Linked selection

Export figures

Export CSV

Generate report

---

## Completion Criteria

The application produces publication-ready outputs.

---

# Version 0.9 — Batch Processing

Goal

Automate repetitive work.

## Features

Batch masking

Batch prediction

Progress window

Processing queue

Error handling

Continue after failure

Result summary

---

## Completion Criteria

Entire folders can be processed automatically.

---

# Version 1.0 — Release

Goal

Stable software for everyday use.

## Features

Installer

Settings

User preferences

Documentation

Example datasets

Model management

Logging

Crash reporting

Automatic updates

---

## Future Ideas

---

### Online Learning

Model updating

Incremental training

Version management

---

### Advanced Visualization

RGB composites

False-color composites

3D visualization

---

### Analysis Tools

PCA visualization

PLS visualization

Feature importance

Band selection

Distance metrics

---

### Export

PNG

TIFF

PDF

CSV

Excel

JSON

---
