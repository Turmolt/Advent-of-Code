(ns adventofcode.util
  (:require [clojure.java.io :as io]
            [clojure.string :as str]))

(defn path [d]
  (str "input/day" d ".txt"))


;TODO: merge two methods below
(defn input-lcsv [d]
  (map #(str/split % #",") (str/split (slurp (str "input/day" d ".txt")) #"\n")))

(defn input-csv [d]
  (vec (map read-string (str/split (slurp (str (path d))) #","))))

(defn input-lsv [d]
  (with-open [r (io/reader (path d))]
    (doall (line-seq r))))