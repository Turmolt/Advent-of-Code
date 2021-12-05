(ns adventofcode.2021.day4
  (:require [adventofcode.util :as u]
            [clojure.string :as str]
            [clojure.set :as set]))

(def data (filter not-empty (u/input-lsv 2021 4)))

(def drawn-numbers (mapv #(Integer/parseInt %) (str/split (first data) #",")))

(def boards (vec (partition 5 (rest data))))
(defn convert-board [input]
  (mapv (partial mapv #(Integer/parseInt %))
        (mapv #(str/split (str/replace (str/trim %) "  " " ") #" ") input)))

(def board-numbers (mapv convert-board boards))

(defn diagonal [input]
  (mapv (partial map-indexed
                 (fn [idx val]
                   [(nth val idx) (nth val (- 4 idx))])) input))

(def compiled-diagonals (diagonal board-numbers))

(defn extract-diagonals [input] [(mapv first input) (mapv second input)])

(def extracted-diagonals (mapv extract-diagonals compiled-diagonals))

(defn extract-verticals [input] (map-indexed (fn [idx val] (mapv #(nth % idx) input)) input))

(def extracted-verts (map extract-verticals board-numbers))

(def compiled-boards (mapv 
                      (fn [b1 b2] 
                        {:board b2
                         :winnables b1})
                      (mapv concat extracted-diagonals extracted-verts board-numbers) board-numbers))

(defn check-if-match [pulled input]
  (every? #(contains? (set pulled) %) (set input)))

(defn check-board-for-match [pulled input]
  (first (filter true? (mapv (partial check-if-match pulled) (:winnables input)))))

(defn filter-boards-for-match [pulled]
  (->> compiled-boards
       (filter #(check-board-for-match pulled %))
       (first)))

(defn pull-until-match []
  (loop [pulled (take 5 drawn-numbers) remaining (drop 5 drawn-numbers)]
    (let [matches (filter-boards-for-match pulled)]
      (if (some? matches)
        {:match matches
         :numbers pulled}
        (recur (conj pulled (first remaining)) (rest remaining))))))

(defn score-board [target]
  (let [board-nums (flatten (:board (:match target)))
        unique (set/difference (set board-nums) (set (:numbers target)))]
    (println board-nums)
    (println unique)
    (* (first (:numbers target)) (reduce + unique))))

(defn solve-part1 []
  (let [match (pull-until-match)]
    (score-board match)))

(comment (time (solve-part1)))
;; => "Elapsed time: 91.9568 msecs"
;; => 5685

;part 2
(defn filter-boards-for-nonmatch [pulled]
  (->> compiled-boards
       (filter (comp not #(check-board-for-match pulled %)))))

(defn pull-until-one-nonmatch []
  (loop [pulled (take 5 drawn-numbers) remaining (drop 5 drawn-numbers) last-winner []]
    (let [matches (filter-boards-for-nonmatch pulled)]
      (if (= 0 (count matches))
        {:match last-winner
         :numbers pulled}
        (recur (conj pulled (first remaining)) (rest remaining) (first matches))))))

(defn solve-part2 []
  (let [last-nonmatch (pull-until-one-nonmatch)]
    (score-board last-nonmatch)))

(comment (time (solve-part2)))
;; => "Elapsed time: 1035.3869 msecs"
;; => 21070