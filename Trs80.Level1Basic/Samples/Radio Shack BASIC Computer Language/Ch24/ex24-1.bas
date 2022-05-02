10 Y=1:N=0
20 in. "Is gate 'A' open";A
30 in. "Is gate 'B' open";B
40 in. "Is gate 'C' open";C
50 print
60 if (A=1) + (B=1) + (C=1) then 100
70 p. "Old Bessie is secure in pasture #1"
80 end
100 p. "A gate is open. Old Bessie is free to roam."