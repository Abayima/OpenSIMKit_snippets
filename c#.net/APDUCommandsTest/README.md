APDU Command Test
=================
Simple test harness for running APDU commands over PC/SC interface. The code relies on the winscard.dll lib from MS and has two .NET wrapper included:

1. ModWinsCard - thanks to chenguo for the FOSS code [here](http://code.google.com/p/chenguo/source/browse/trunk/DriverTest/DriverTest/ModWinsCard.cs?spec=svn262&r=262).

2. GemCard - widely available APDU winscard.dll wrapper, not sure where it came from :/

The body of code is borrowed from [this](http://justmycode.blogspot.com/2009/07/smart-cards-hurt-2.html) great article - thanks, [Evgeny Rokhlin](https://plus.google.com/112677661119561622427)
