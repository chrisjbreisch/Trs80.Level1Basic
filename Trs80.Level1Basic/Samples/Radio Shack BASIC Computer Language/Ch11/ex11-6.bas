10 rem * finds optimum load to source match *
20 cls
30 print "LOAD ", "CIRCUIT ", "SOURCE ", "LOAD "
40 print "RESISTANCE ", "POWER ", "POWER ", "POWER "
50 print "(OHMS)", "(WATTS)", "(WATTS)", "(WATTS)"
60 print
70 for r=1 to 20
80   i = 120/(10+r)
90   c = i*i*(10+r)
100  s = i*i*10
110  l = i*i*r
120  print r,c,s,l
130 next r