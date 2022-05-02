1 cls
10 for a=1 to 360
20    x=a:gosub 30370
30    y=y*20
40    set(a/3,y+22)
50 next a
60 goto 60

30000 end
30370 rem * sin * input x in degrees, output y
30371 rem also uses z internally
30376 z=abs(x)/x:x=z*x
30380 if x>360 t. x=x/360 : x=(x-int(x))*360
30390 if x>90t.x=x/90:y=int(x):x=(x-y)*90:onyg.30410,30420,30430
30400 x=x/57.29578 : if abs(x)<2.48616e-4 y=0: ret.
30405 g.30440
30410 x=90-x : g.30400
30420 x=-x : g.30400
30430 x=x-90 : g.30400
30440 y=x-x*x*x/6+x*x*x*x/120-x*x*x*x*x*x/5040
30450 y=y+x*x*x*x*x*x*x*x*x/362880 : if z=-1t. y=-y
30455 ret.