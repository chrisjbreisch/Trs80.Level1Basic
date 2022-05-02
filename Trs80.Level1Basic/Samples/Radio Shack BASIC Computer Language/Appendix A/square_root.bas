30000 end
30010 rem * square root* input x, output y
30020 rem also uses w & z internally
30025 rem Added unit tests to original. Use: run 32000
30030 if x = 0 t. y = 0: ret.
30040 if x>0 t. 30060
30050 p. "Root of negative number?": stop
30060 y=x*.5 : z = 0
30070 w=(x/y-y)*.5
30080 if (w=0) + (w=z) t. ret.
30090 y=y+w : z=w : g.30070

31999 end
32000 cls
32005 x=4 : gosub 30010
32010 p."SQRT(4)--Expected: 2, Actual:";y
32015 x=9 : gosub 30010
32020 p."SQRT(9)--Expected: 3, Actual:";y
32025 x=2 : gosub 30010
32030 p."SQRT(2)--Expected: 1.41421, Actual:";y
32035 x=100 : gosub 30010
32040 p."SQRT(100)--Expected: 10, Actual:";y
32045 x=1024 : gosub 30010
32050 p."SQRT(1024)--Expected: 32, Actual:";y
