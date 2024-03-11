# StaticFieldEpidEval: A tool for point evaluation of static photon fields for portal dosimetry using the EPID

A simple point evaluation tool for portal dosimetry using the EPID. It is intended to be used for static fields only.
Replaces the legacy method of using in vivo dosimetry with diodes for static fields.

## Installation

Dependencies:

```VMS.CA.Scripting
```VMS.DV.PV.Scripting
```PortalDosimetry

Runs as a script in Portal Dosimetry module in Aria, in the Tools menu when a portal dosimetry image is in context.


## Usage

Expects a csv text section in clipboard with the following format:

```INVIVO_START
```FieldID,FieldSize,SSD,Depth
```1,10x10,100,10
```2,20x20,100,10
```3,30x30,100,10
```INVIVO_END
```1,2.5,3.0,2.45
```INVIVO_END


## License

TBD ( maybe [MIT](https://choosealicense.com/licenses/mit/) or something else)