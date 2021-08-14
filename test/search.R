require(JSON);

options(http.cache_dir = `${dirname(@script)}/.cache/`);

const result = BingAcademic::search("aes");
const json = json_encode(result);

print(json);

str(result[1]);

writeLines(json, con = `${dirname(@script)}/test.json`);