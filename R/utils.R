const frameData as function(list, datatag = "name") {
    data.frame(
        id   = sapply(list, i -> i$id),
        term = sapply(list, i -> i[[datatag]]) 
    );
}