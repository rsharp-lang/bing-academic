imports "package_utils" from "devkit";

require(JSON);

setwd(dirname(@script));
# package_utils::attach("../");
options(http.cache_dir = "./.cache_html/");

for (i in 1:300) {
    const result = BingAcademic::search("flux balance analysis", page = i);
    const json = json_encode(result, indent = TRUE);

    cat(".");
    writeLines(json, con = `./FBA/page_${i}.json`);
}