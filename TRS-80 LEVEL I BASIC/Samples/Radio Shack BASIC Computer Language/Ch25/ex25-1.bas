10 input "Distance from tree";d
20 input "Height of the tree you're seeking";h
30 x=h/d:gosub 30660
40 print "Required angle is ";c;"degrees."

30000 end
30660 rem * arctangent * input x, output c, a
30670 rem c is in degrees, a is in radians
30680 rem also uses b, t internally
30690 gos. 30810 : x=abs(x) : c=0
30700 if x>1 t. c=1 : x=1/x
30710 a=x*x
30720 b=((2.86623e-3*a-1.61657e-2)*a+4.29096e-2)*a
30730 b=((((b-7.5289e-2)*a+.106563)*a-.142089)*a+.199936)*a
30740 a=((b-.333332)*a+1)*x
30750 if c=1 t. a=1.570796-a
30760 a=t*a : c=a*57.29578 : ret.

30000 end
30800 rem * sign * input x, output t=-1,0,+1
30810 if x < 0 t. t=-1
30820 if x = 0 t. t=0
30830 if x > 0 t. t=1
30840 ret.