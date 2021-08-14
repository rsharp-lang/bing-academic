imports "graphquery" from "webKit";

#' Query html document
#' 
#' @description Parse the given html document text and 
#'    query of the html components as R object.
#' 
#' @param html the html document text
#' @param queryName the relative file name of the 
#'    graphquery definition file. 
#' 
#' @return a list/vector of html document element query result output
#' 
const html_query as function(html, queryName) {
    Html::parse(html)
    |> query(
        graphquery = system.file(queryName, package = "BingAcademic")
                    |> readText
                    |> parseQuery
    )
    ;
}