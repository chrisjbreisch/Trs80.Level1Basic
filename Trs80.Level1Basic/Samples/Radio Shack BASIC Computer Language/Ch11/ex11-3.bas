5 cls
10 print "     ***   S A L A R Y  R A T E  C H A R T   ***"
20 print
30 print "YEAR ", "MONTH ", "WEEK ", "DAY "
40 print
50 for y = 5000 to 25000 step 1000
55   rem *convert yearly income into monthly*
60   m=y/12
65   rem *convert yearly income into weekly*
70   w=y/52
75   rem *convert weekly income into daily*
80   d=w/5
100  print y, m, w, d
110 next y