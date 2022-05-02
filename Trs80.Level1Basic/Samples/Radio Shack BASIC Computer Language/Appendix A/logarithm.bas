30000 end
30170 rem * natural & common log* input x, output l
30175 rem output l is natural log, output x is common log
30180 rem also uses a,b,c internally
30185 rem Added unit tests to original. Use: run 32000
30190 e=0 : if x<0 t. p. "Log undefined at";x:stop
30195 a=1 : b=2 : c=.5
30200 if x>=a t. x=c*x : e=e+a : g. 30200
30205 if x<c t. x=b*x : e=e-a : g. 30205
30210 x=(x-.707107)/(x+.707107) : l=x*x
30215 l=(((.598979*l+.961471)*l+2.88539)*x+e-.5)*.693147
30220 if abs(l)<1e-6 t. l=0
30225 x=l*.4342945 : ret.

31999 end
32000 cls
32105 x=1 : gosub 30190
32110 p."LOG(1)--Expected: [0 0], Actual:[";l;x;"]"
32115 x=2.718282 : gosub 30190
32120 p."LOG(2.718282)--Expected: [1 0.434295], Actual:[";l;x;"]"
32125 x=10 : gosub 30190
32130 p."LOG(10)--Expected: [2.30259 1], Actual:[";l;x;"]"
32135 x=100 : gosub 30190
32140 p."LOG(100)--Expected: [4.60517 2], Actual:[";l;x;"]"
32145 x=2 : gosub 30190
32150 p."LOG(2)--Expected: [0.693147 0.30103], Actual:[";l;x;"]"
32155 x=4 : gosub 30190
32160 p."LOG(4)--Expected: [1.38629 0.60206], Actual:[";l;x;"]"

