imports ["Html", "http", "graphquery"] from "webKit";

#' Do bing academic search
#' 
#' @param term the term string to be searched
#' 
#' @return a list of term search result which is query from 
#'     the bing academic search engine.
#' 
const search as function(term, page = 1) {
    const urlq as string  = `https://cn.bing.com/academic/search?q=${urlencode(term)}&qs=HS&sc=8-5&cvid=8B323F881DD4441999B907EA555EC5F0&first=${1 + as.numeric(page - 1) * 10}&FORM=PENR`;
    const html as string = REnv::getHtml(urlq);
    const result = html
    |> BingAcademic::html_query("graphquery/listPage.graphquery")
    |> sapply(function(i) {
		const citeNumber as integer = {
			let test = i$publication$cites == $"\d+";
		
			if (length(test) == 0) {
				test = FALSE;
			}
		
			if (test) {
				as.integer(i$publication$cites);
			} else {
				0;
			}
		};
	
        .summary(
            title    = i$title$title, 
            guid     = i$title$guid, 
            ref      = i$publication$ref, 
            cites    = citeNumber, 
            abstract = i$abstract,
            authors  = frameData(i$authors),
            fields   = frameData(i$fields, "term")
        );
    })
    ;

    result;
}

