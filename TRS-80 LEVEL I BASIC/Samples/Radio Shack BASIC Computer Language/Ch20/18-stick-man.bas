10 cls
20 x=64
30 for k=0 to 7
40   set(x+k, 40+k)
45   reset(x+k-1, 40+k)
50   set(x-k, 40+k)
55   reset(x-k-1,40+k)
60 next k
70 for k=0 to 5
80   set(x+k,34+k)
85   reset(x+k-1,34+k)
90   set(x,34+k)
95   reset(x-1,34+k)
100  set(x-k,34+k)
105  reset(x-k-1,34+k)
110 next k
120 set(x,32)
125 reset(x-1,32)
130 set(x+1,33)
135 reset(x,33)
140 set(x-1,33)
145 reset(x-2, 33)
150 x=x+1
160 goto 30
999 goto 999