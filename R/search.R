imports ["Html", "http", "graphquery"] from "webKit";

#' Do bing academic search
#' 
#' @param term the term string to be searched
#' 
#' @return a list of term search result which is query from 
#'     the bing academic search engine.
#' 
const search as function(term) {
    const urlq as string  = `https://cn.bing.com/academic/search?q=${urlencode(term)}&first=${urlencode(term)}&FORM=HDRSC4`;
    const html as string = REnv::getHtml(urlq);
    const result = html
    |> BingAcademic::html_query("graphquery/listPage.graphquery")
    |> sapply(function(i) {
        .summary(
            title    = i$title$title, 
            guid     = i$title$guid, 
            ref      = i$publication$ref, 
            cites    = i$publication$cites, 
            abstract = i$abstract,
            authors  = frameData(i$authors),
            fields   = frameData(i$fields, "term")
        );
    })
    ;

    result;
}

