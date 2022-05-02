10 in. "How far are you from base of tree";d
20 in. "What is angle between tip and base of tree";a
30 x=a:gosub 30320
40 h=int(d*y+.5)
50 if h=28 then 80
60 p. "Find another tree--this one is";h;"feet tall."
70 p.:goto 10
80 p. "Chop it down and take it home!"

30000 end
30300 rem * tangent * input x in degrees, output y
30310 rem also uses a,c,w,z internally
30320 a=x : gos. 30360
30330 if abs(y)<1e-5 t. p. "Tangent undefined": stop
30340 c=y : x=a : gos.30376 : y=y/c : ret.

30000 end
30350 rem * cosine * input x in degrees, output y
30351 rem also uses w,z internally
30360 w=abs(x)/x:x=x+90:gos.30376:if(z=-1)*(w=1)t.y=-y
30365 ret.

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