10 print "Seeking the value of x to the y power"
20 input "X = ";x
30 input "Y = ";y
40 gosub 30120
50 print "The answer is";p
60 goto 10

30000 end
30100 rem * exponentiation* input x,y: output p
30110 rem also uses e,l,a,b,c internally
30120 p=1 : e=0 : if y = 0 t. ret.
30130 if (x<0)*(int(y)=y) t. p=1-2*y+4*int(y/2) : x=-x
30140 if x<>0 t. gos. 30190 : x=y*l : gos. 30250
30150 p=p*e : ret.

30000 end
30170 rem * natural & common log* input x, output l
30175 rem output l is natural log, output x is common log
30180 rem also uses a,b,c internally
30190 e=0 : if x<0 t. p. "Log undefined at";x:stop
30195 a=1 : b=2 : c=.5
30200 if x>=a t. x=c*x : e=e+a : g. 30200
30205 if x<c t. x=b*x : e=e-a : g. 30205
30210 x=(x-.707107)/(x+.707107) : l=x*x
30215 l=(((.598979*l+.961471)*l+2.88539)*x+e-.5)*.693147
30220 if abs(l)<1e-6 t. l=0
30225 x=l*.4342945 : ret.

30000 end
30240 rem * exponential * input x, output e
30245 rem also uses l,a internally
30250 l=int(1.4427*x)+1 : if l<127 t. 30265
30255 if x>0 t. p. "Overflow " : stop
30260 e=0 : ret.
30265 e=.693147*l-x : a=1.32988e-3-1.41316e-4*e
30275 e=(((a-.166665)*e+.5)*e-1)*e+1 : a=2
30280 if l<=0 t. a=.5 : l=-l : if l=0 t. ret.
30285 f. x=1 to l : e=a*e : n. x : ret.
