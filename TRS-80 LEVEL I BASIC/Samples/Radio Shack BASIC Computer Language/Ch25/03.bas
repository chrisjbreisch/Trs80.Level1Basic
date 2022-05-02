10 cls
20 for r=2 to 22 step 4
30    for a=-r to r
40       x=r*r-a*a : gosub 30030 : y=int(y-.5)
50       set(a+60,23+y)
60       set(a+60,23-y)
70    next a
80 next r
90 goto 90


30000 end
30010 rem * square root* input x, output y
30020 rem also uses w & z internally
30030 if x = 0 t. y = 0: ret.
30040 if x>0 t. 30060
30050 p. "Root of negative number?": stop
30060 y=x*.5 : z = 0
30070 w=(x/y-y)*.5
30080 if (w=0) + (w=z) t. ret.
30090 y=y+w : z=w : g.30070