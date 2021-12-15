(ns adventofcode.util
  (:require [clojure.java.io :as io]
            [clojure.string :as str]))

(defn path [y d]
  (str y "/input/day" d ".txt"))

(defn input [y d] (slurp (path y d)))

;TODO: merge two methods below
(defn input-lcsv [y d]
  (map #(str/split % #",") (str/split-lines (input y d))))

(defn input-csv [y d]
  (vec (map read-string (str/split (input y d) #","))))

(defn input-csv-long [y d]
  (vec (map #(Long/parseLong %) (str/split (input y d) #","))))

(defn input-lsv [y d]
  (with-open [r (io/reader (path y d))]
    (doall (line-seq r))))

(defn find-first [pred coll] (first (filter pred coll)))

(defn gcd [x y] (if (zero? y) x (recur y (mod x y))))

(defn lcm [x y] (/ (* x y) (gcd x y)))
