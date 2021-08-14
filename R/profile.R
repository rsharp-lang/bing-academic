
#' Get profile of the details information about the given literature entry
#'
#' @description the information of the details contains reference list, 
#'    cites data, DOI, etc...
#' 
#' @param literature the literature term entry from the search result or 
#'    the bing guid of the literature.
#' 
#' @return the details information of the given article.
#' 
const profile as function(literature) {
    const guid as string = get_guid(literature);
    const url as string = `https://cn.bing.com/academic/profile?id=${guid}&encoded=0&v=paper_preview&mkt=zh-cn`;
    const html as string = REnv::getHtml(url);
    const information = BingAcademic::html_query(html, "graphquery/literature.graphquery");

    information;
}

const get_guid as function(literature) {
    if (as.object(typeof(literature))$name == "literatureEntry") {
        as.object(literature)$guid;
    } else {
        if (typeof literature == "string") {
            literature;
        } else {
            literature$guid;
        }
    }
}