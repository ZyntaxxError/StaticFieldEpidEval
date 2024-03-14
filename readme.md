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

In portal dosimetry with one of the images that should be evaluated in context, select the Tools menu and select StaticFieldEpidEval.


Expects a csv text section in clipboard with the following format:

For online or inVivo measurements:
Treatment plan UID
FieldID,Read out position in collimator coordinates [IEC61217] in mm [double,double], expected value in CU (calibrated units) [double]
INVIVO_END

For offline or inVitro measurements with a verification plan created:
Verification plan UID
FieldID,Read out position in collimator coordinates [IEC61217] in mm [double,double], expected value in CU (calibrated units) [double]
INVITRO_END

Example:
INVIVO
P1 TestPlan
1.2.246.352.71.5.375632402357.483435.20240116101721
1,0.00,-30.00,0.050
2,0.00,0.00,0.032
3,0.00,0.00,0.089
END_INVIVO





## Structure and function

Searches for the UID belonging to the contexts image plan in the clipboard text, 







## License

TBD ( maybe [MIT](https://choosealicense.com/licenses/mit/) or something else)