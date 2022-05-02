10 cls
20 x=64
30 for k=0 to 7
40   set(x+k, 40+k)
50   set(x-k, 40+k)
60 next k
70 for k=0 to 5
80   set(x+k,34+k)
90   set(x,34+k)
100  set(x-k,34+k)
110 next k
999 goto 999