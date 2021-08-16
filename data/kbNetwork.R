require(JSON);
require(igraph);

const g = empty.network();
const loading = list.files(`${dirname(@script)}/FBA`, pattern = "*.json") 
|> lapply(function(path) {
	json_decode(readText(path))
})
;

# str(loading);

for(list in loading) {
	for(article in list) {
		const words = article$keywords;
		
		str(words);
		
		for(word in words) {
			if (length(g |> getElementByID(word$word)) == 0) {
				g :> add.node(label = word$word);
			}
			
			for(word2 in words) {
				if (word$word != word2$word) {
					if (!(g |> has.edge(word$word, word2$word))) {
						g |> add.edge(word$word, word2$word);
					}
					
					g |> weight(word$word, word2$word, g |> weight(word$word, word2$word) + 1);
				}
			}
		}		
	}	
}

print(g);

g 
|> louvain_cluster
|> save.network(`${dirname(@script)}/FBA_graph/`)
;

