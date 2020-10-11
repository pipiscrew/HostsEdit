HostsEdit
---------

-When press save, the previous hosts file copied to hosts.pc.bak.

-To search, type keyword and press ENTER

-Grid context menu has four options :
	1-copy selected (multiple rows supported)
	2-delete selected (multiple rows supported)
	3-consolidate selected (multiple rows required)
	4-divide selected (signle row required)

by default the HOSTS file accepts multiple hosts in one line. The separator is the space. So for example we can have :

127.0.0.1 google.com yahoo.com yandex.com

the context menu options 3 + 4, coming to do modification according this (multiple hosts in one line).

the 3, for example you have selected the following rows :
	127.0.0.1 google.com
	127.0.0.1 yahoo.com
	127.0.0.1 yandex.com

	and press 'consolidate selected', will remove the selected last rows and combine it to first, so will be :
	127.0.0.1 google.com yahoo.com yandex.com

the 4, for example you have select a row contains :
	127.0.0.1 google.com yahoo.com yandex.com

	and press 'divide selected',will create two new rows, so will end up as :
	127.0.0.1 google.com
	127.0.0.1 yahoo.com
	127.0.0.1 yandex.com
	

known issue, the application doesnt respect remarks after the host

if you have troubles, run it as administrator.