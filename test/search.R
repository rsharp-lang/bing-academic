require(JSON);

options(http.cache_dir = `${dirname(@script)}/.cache/`);

const result = BingAcademic::search("flux balance analysis");
const json = json_encode(result[1], indent = TRUE);

cat("\n\n");
console::log(json);

# str(result[1]);

# writeLines(json, con = `${dirname(@script)}/test.json`);

# print(profile(result[1]));