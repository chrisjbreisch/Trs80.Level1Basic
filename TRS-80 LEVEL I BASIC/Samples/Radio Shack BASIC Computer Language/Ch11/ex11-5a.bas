5 cls
9 rem *set maximum area at zero*
10 m=0
14 rem *set desired length at zero*
15 n=0
19 rem *f is total fee of fence available*
20 f=1000
24 rem *l is length of one side of rectangle*
25 for l=0 to 500 step 50
29   rem *w is width of one side of rectangle*
30   w=(f-2*l)/2
35   a=w*l
39   rem *compare a with current maximum, replace if necessary*
40   if a<=m then goto 55
45   m=a
49   rem *also update current desired length*
50   n=l
55 next l
60 print "FOR LARGEST AREA USE THESE DIMENSIONS:"
65 print n; "FT. BY";500-N;"FT. FOR TOTAL AREA OF";m;"SQ. FT."
