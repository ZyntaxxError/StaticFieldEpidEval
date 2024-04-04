# StaticFieldEpidEval: A tool for point evaluation of static photon fields for portal dosimetry using the EPID

A simple point evaluation tool for portal dosimetry using the EPID. It is intended to be used for static fields only.
Replaces the legacy method of using in vivo dosimetry with diodes for static fields.

## Installation

Dependencies:

VMS.DV.PV.Scripting   
VMS.CA.Scripting

Runs as a script in Portal Dosimetry module in Aria 16.1  
Requires a cvs-formatted text in System.Windows.Forms.Clipboard with the expected values for the fields to be evaluated.


## Usage

In portal dosimetry with one of the images that should be evaluated in context, select the Tools menu and select StaticFieldEpidEval.


Expects a csv text section in clipboard with the following format:

For online or inVivo measurements:  
Treatment plan UID  
FieldID [int],Read out position in collimator coordinates (IEC61217) in mm [double,double], expected value in CU (calibrated units) [double]  
INVIVO_END  

For offline or inVitro measurements with a verification plan created:  
Verification plan UID  
FieldID [int],Read out position in collimator coordinates [IEC61217] in mm [double,double], expected value in CU (calibrated units) [double]  
INVITRO_END  

### Example:  
INVIVO  
P1 TestPlan  
1.2.246.352.71.5.375632402357.483435.20240116101721  
1,0.00,-30.00,0.050  
2,0.00,0.00,0.032  
3,0.00,0.00,0.089  
END_INVIVO  


If the beam is interrupted, and more than one image is acquired, create a composite image before running the script.



## Structure and function

Retrieves the image plan UID from the context, and searches for the corresponding csv text in the clipboard.
For each field in the csv, it retrieves the image from the context, and evaluates the field at the specified position.




