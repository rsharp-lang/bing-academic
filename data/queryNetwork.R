imports "package" from "devkit";

require(JSON);

setwd(dirname(@script));
package::attach("../");
options(http.cache_dir = "./.cache/");

for (i in 1:100) {
    const result = BingAcademic::search("flux balance analysis", page = i);
    const json = json_encode(result[1], indent = TRUE);

    cat(".");
    writeLines(json, con = `./FBA/page_${i}.json`);
}