(ns adventofcode.2021.day2
  (:require [adventofcode.util :as u]
            [clojure.string :as str]))

(def command {:forward [1 0]
              :up [0 -1]
              :down [0 1]})

(defn parse-data [i] (let [tokens (str/split i #" ")
                           action (command (keyword (first tokens)))
                           distance (Integer/parseInt (second tokens))]
                       (mapv (fn [i] (* distance i)) action)))

(def data (mapv parse-data (u/input-lsv 2021 2)))

(def reduced-data (reduce (partial mapv +) data))

(def answer-part1 (reduce * reduced-data))

;part 2
(defn solve [] (loop [horizontal 0 depth 0 aim 0 current (first data) remaining (rest data)]
                 (let [h (first current) a (second current)
                       new-aim (+ aim a)
                       new-depth (+ depth (* new-aim h))
                       new-horizontal (+ horizontal h)]
                   (if (not (nil? (first remaining)))
                     (recur new-horizontal new-depth new-aim (first remaining) (rest remaining))
                     [new-horizontal new-depth]))))


(def answer-part2 (reduce * (solve)))