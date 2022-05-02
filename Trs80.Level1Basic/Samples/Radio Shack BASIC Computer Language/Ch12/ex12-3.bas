10 rem * finds optimum load to source match *
20 cls
30 print "INTER ";tab(10);"LOAD";tab(21);"CIRCUIT ";
35 print tab(36);"SOURCE ";tab(51);"LOAD "
40 print "RESIST ";tab(10);"RESIST ";tab(21);"POWER ";
45 print tab(36);"POWER ";tab(51);"POWER "
50 print " (OHMS)";tab(10);"(OHMS)";tab(21);"(WATTS)";
55 print tab(36);"(WATTS)";tab(51);"(WATTS)"
60 print
70 for r=1 to 20
80   i = 120/(10+r)
90   c = i*i*(10+r)
100  s = i*i*10
110  l = i*i*r
120  print "   10";tab(10);r;tab(20);c;tab(35);s;tab(50);l
130 next r