30000 end
30240 rem * exponential * input x, output e
30245 rem also uses l,a internally
30247 rem Added unit tests to original. Use: run 32000
30250 l=int(1.4427*x)+1 : if l<127 t. 30265
30255 if x>0 t. p. "Overflow " : Stop
30260 e=0 : ret.
30265 e=.693147*l-x : a=1.32988e-3-1.41316e-4*e
30275 e=(((a-.166665)*e+.5)*e-1)*e+1 : a=2
30280 if l<=0 t. a=.5 : l=-l : if l=0 t. ret.
30285 f. x=1 to l : e = a * e : n.x : ret.

31999 end
32000 cls
32205 x=1 : gosub 30250
32210 p."EXP(1)--Expected: 2.71829, Actual:";e
32215 x=0 : gosub 30250
32220 p."EXP(0)--Expected: 1, Actual:";e
32225 x=2 : gosub 30250
32230 p."EXP(2)--Expected: 7.38906, Actual:";e
32235 x=4 : gosub 30250
32240 p."EXP(4)--Expected: 54.5982, Actual:";e
32245 x=2.71829 : gosub 30250
32250 p."EXP(2.71829)--Expected: 15.1544, Actual:";e
