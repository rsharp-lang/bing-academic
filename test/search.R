require(JSON);

options(http.cache_dir = `${dirname(@script)}/.cache/`);

const result = BingAcademic::search("flux balance analysis");
const json = json_encode(result);

print(json);

str(result[1]);

# writeLines(json, con = `${dirname(@script)}/test.json`);

print(profile(result[1]));