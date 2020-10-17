HostsEdit
---------

-When press save, the previous hosts file copied to hosts.pc.bak.

-To search, type keyword and press ENTER

-Grid context menu has five options :
-	1-copy selected (multiple rows supported)
-	2-delete selected (multiple rows supported)
-	3-consolidate selected (multiple rows required)
-	4-divide selected (signle row required)
-	5-divide per 9 hosts (single row required)

by default the HOSTS file accepts multiple hosts in one line (max [9 hosts](https://superuser.com/a/932113)). The separator is the space. So for example we can have :

127.0.0.1 google.com yahoo.com yandex.com

the context menu options 3 + 4, coming to do modification according this (multiple hosts in one line).

the 3, for example you have selected the following rows :
-	127.0.0.1 google.com
-	127.0.0.1 yahoo.com
-	127.0.0.1 yandex.com

	and press 'consolidate selected', will remove the selected last rows and combine it to first, so will be :
	
	`127.0.0.1 google.com yahoo.com yandex.com`

the 4, for example you have select a row contains :
-	127.0.0.1 google.com yahoo.com yandex.com

	and press 'divide selected',will create two new rows, so will end up as :
```
127.0.0.1 google.com
127.0.0.1 yahoo.com
127.0.0.1 yandex.com
```
the 5th option appears when a row has red background (this means that exceed the 9 hosts), so for example you have :
- 127.0.0.1  h01 h02 h03 h04 h05 h06 h07 h08 h09 h10 h11

by the MS limitation, the `h10 h11` doing nothing. has to go to new line, there is the 5th option comes, splits the selected row per 9 hosts.


known issue, the application doesnt respect remarks **after** the host(s)

if you have troubles, run it as administrator.