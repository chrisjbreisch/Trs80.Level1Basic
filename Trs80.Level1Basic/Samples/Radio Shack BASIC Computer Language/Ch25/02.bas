10 rem * square root solution with subroutine *
20 in. "The length of side A = ";a
30 in. "The length of sise B = ";b
40 x = a*a+b*b: gosub 30030
45 l = y
50 p."A","B","L"
60 p. a,b,l
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