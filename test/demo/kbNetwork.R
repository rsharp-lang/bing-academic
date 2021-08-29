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

str(loading[[1]]);

const pushWordVertex as function(word, cites) {
	if (length(g |> getElementByID(word)) == 0) {
		g :> add.node(label = word);
	}
	
	mass(g, word) = {			
		const node  = g |> getElementByID(word);
		const ndata = as.object(as.object(node)$data);

		ndata$mass + ifelse(cites == 0, 1, cites);
	}
}
const pushArticle as function(article) {
	const words as string  = sapply(article$keywords, w -> w$word);
	const cites as integer = as.integer(article$cites);
	const firstWord as string = splitKeyword(words[1]);

	print(article$title);
	pushWordVertex(firstWord, cites);

	for(word in words[2:length(words)]) {
		word = splitKeyword(word);
	
		pushWordVertex(word, cites);

		if (!(g |> has.edge(firstWord, word))) {
			g |> add.edge(firstWord, word);
		}
		
		g |> weight(firstWord, word, g |> weight(firstWord, word) + 1);
	}
}

for(list in loading) {
	for(article in list) {
		pushArticle(article);
	}
}

const q = quantile(unlist(mass(g)));
const i as boolean = unlist(mass(g)) < q[["50%"]];
const j as boolean = unlist(mass(g)) > q[["75%"]];

print("Words of cites count less than 50% quantile:");
print(sum(i));
print("Words of cites count greater than 75% quantile:");
print(sum(j));

mass(g, NULL) = lapply(mass(g), x -> ifelse(x < q[["50%"]], q[["50%"]], x));

g 
|> connected_graph
|> louvain_cluster
|> (function(g) {
	# set node color by class
	class(g) = colors("scibasic.category31()", length(unique(class(g))), character = TRUE)[factor(class(g))];
	g;
})
|> save.network(`${dirname(@script)}/FBA_graph/`)
;

