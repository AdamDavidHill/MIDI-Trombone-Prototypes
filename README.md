MIDI-Trombone-Prototypes
========================
MSc Research Project

Adam Hill

The Open University
========================

This solution is in C# / WPF for Visual Studio 2013. The software is a hybrid of two prototypes created to allow the realtime playing of MIDI data in a trombone-like fashion. One prototype uses a touch interface, the other a Kinect for Windows 2. In order to run the program:

• Install Kinect for Windows 2 SDK: http://www.microsoft.com/en-us/download/details.aspx?id=43661

• Install a MIDI loopback driver, such as http://www.tobias-erichsen.de/software/loopmidi.html

• Install a suitable VMI, such as http://www.samplemodeling.com/en/products_trombone.php

• A breath controller is highly recommended, such as http://tecontrol.se/products/usb-midi-breath-controller

• Launch the loopback driver and create a MIDI port named "VBone"

• Set the VMI input to this MIDI port

• Enable breath control in the VMI if appropriate

• Launch this software

========================

This is prototype software. Use at own risk.

========================

I would like to extend my gratitude to the creators of the libraries and drivers this software is dependent on. In particular, Tobias Erichsen for virtualMIDI and loopMIDI, and Tom Lokovic for midi-dot-net (http://code.google.com/p/midi-dot-net/).
