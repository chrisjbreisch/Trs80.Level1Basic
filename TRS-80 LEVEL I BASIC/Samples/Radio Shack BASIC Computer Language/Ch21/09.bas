30 for c=1 to 52:read a(c) : Next c
50 data 1,2,3,4,5,6,7,8,9,10,11,12,13,14,15,16,17,18,19,20
55 data 21,22,23,24,25,26,27,28,29,30,31,32,33,34,35
60 data 36,37,38,39,40,41,42,43,44,45,46,47,48,49,50,51,52
90 p = 52
100 v = rnd(52)
105 if a(v) = 0 g.100
110 print a(v)
120 a(v) = 0
130 p = p - 1
140 if p <> 0 goto 100
150 print "End of deck!"
200 for t = 1 to 52
210   print a(t);
220 next t