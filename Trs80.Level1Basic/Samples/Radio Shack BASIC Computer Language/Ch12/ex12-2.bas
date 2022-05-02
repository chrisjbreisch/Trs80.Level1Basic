5 cls
10 print "     ***   S A L A R Y  R A T E  C H A R T   ***"
20 print
30 print tab(1);"YEAR ";tab(12);"MONTH ";tab(25);"WEEK ";
40 print tab(38);"DAY ";tab(51);"HOUR "
50 for y = 5000 to 25000 step 1000
55   rem *convert yearly income into monthly*
60   m=y/12
65   rem *convert yearly income into weekly*
70   w=y/52
75   rem *convert weekly income into daily*
80   d=w/5
85   rem-convert weekly income into hourly
90   h=w/40
100  print y;tab(11);m;tab(24);w;tab(37);d;tab(50);h
110 next y