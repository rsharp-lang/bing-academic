require(JSON);
require(igraph);

const g = empty.network();
const loading = list.files(`${dirname(@script)}/FBA`, pattern = "*.json") 
|> lapply(function(path) {
	json_decode(readText(path))
})
;
const equals_FBA as string = [
	"Flux Balance Analysis",
	"FluxBalanceAnalysis",
	"Flux BalanceAnalysis"
];
const splitKeyword as function(str) {
	if (any(str == equals_FBA)) {
		"FBA";
	} else {
		str;
	}
}

const pushArticle as function(article) {
	const words            = article$keywords;
	const cites as integer = as.integer(article$cites);

	print(article$title);

	for(word in words) {
		word = splitKeyword(word$word);
	
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
			word2 = splitKeyword(word2$word);

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

mass(g, NULL) = lapply(mass(g), log);

const i as boolean = unlist(mass(g)) < quantile(unlist(mass(g)))[["50%"]];

print("low cites words will be removes from the word graph:");
print(sum(i));
print(i);

for(v in vertex(g)[i]) {
	g |> igraph::delete(v);
}

g 
|> connected_graph
|> louvain_cluster
|> (function(g) {
	class(g) = colors(length(unique(class(g))))[factor(class(g))];
	g;
})
|> save.network(`${dirname(@script)}/FBA_graph/`)
;

