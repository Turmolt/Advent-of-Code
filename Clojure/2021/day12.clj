(ns adventofcode.2021.day12
  (:require [clojure.string :as str]))

(let [input (->> (slurp "2021/input/day12.txt") (str/split-lines))
      in-split (map #(str/split % #"-") input)
      both-dir (mapcat #((juxt identity reverse) %) in-split)
      grouped (group-by first both-dir)
      clean-up-val (fn [[k v]] (filter #(not= % "start") (map second v)))
      grouped-clean (reduce #(assoc %1 (first %2) (clean-up-val %2)) {} grouped)]
  (def out-edges grouped-clean))

(defn valid [puzzle1?]
  (fn [path target]
    (or (= target (str/upper-case target))
        (not-any? #{target} path)
        (when-not puzzle1? (apply distinct? (filter #(= %1 (str/lower-case %1)) path))))))

(defn walk [node path valid-fn]
  (if (= node "end")
    1
    (let [targets (filter (partial valid-fn path) (get out-edges node))]
      (reduce + (map #(walk %1 (conj path %1) valid-fn) targets)))))

(println "Puzzle1 " (walk "start" ["start"] (valid true)))
(println "Puzzle2 " (walk "start" ["start"] (valid false)))