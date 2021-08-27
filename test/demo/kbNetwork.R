require(JSON);
require(igraph);

const g = empty.network();
const loading = list.files(`${dirname(@script)}/FBA`, pattern = "*.json") 
|> lapply(function(path) {
	json_decode(readText(path))
})
;
const pushArticle as function(article) {
	const words            = article$keywords;
	const cites as integer = as.integer(article$cites);

	print(article$title);

	for(word in words) {
		word = word$word;
	
		if (length(g |> getElementByID(word)) == 0) {
			g :> add.node(label = word);
		}
		
		# print(word);

		mass(g, word) = {			
			const node  = g |> getElementByID(word);
			const ndata = as.object(as.object(node)$data);

			# print(names(ndata));

			ndata$mass + ifelse(cites == 0, 1, cites);
		}

		for(word2 in words) {
			word2 = word2$word;

			if (word != word2) {
				if (!(g |> has.edge(word, word2))) {
					g |> add.edge(word, word2);
				}
				
				g |> weight(word, word2, g |> weight(word, word2) + 1);
			}
		}
	}
}

for(list in loading) {
	for(article in list) {
		pushArticle(article);
	}	
}

const i as boolean = unlist(mass(g)) < quantile(unlist(mass(g)))[["25%"]];

print("low cites words will be removes from the word graph:");
print(sum(i));
print(i);

for(v in vertex(g)[i]) {
	g |> igraph::delete(v);
}

g 
|> louvain_cluster
|> save.network(`${dirname(@script)}/FBA_graph/`)
;

