imports "search" from "../dist/academic.dll";

let data = search("aes")
:> as.data.frame
;

data
:> print
;
data 
:> write.csv(file = "../data/demo_search.csv")
;
