(ns adventofcode.day4)

(def re ['#"(\d)\1{1,}" '#"^(?=\d{6}$)0*?1*?2*?3*?4*?5*?6*?7*?8*?9*?$"])

(defn legal? [i]
  (not (nil? (some #(= 2 (count (str (first %)))) i))))

(defn op [i]
  (if (= i (first re)) legal? #(pos? (count %))))

(defn fits-all? [i n]
  (every? (partial (partial #((op %) (re-seq % (str n))))) i))

(println (count (transduce (filter (partial fits-all? re)) conj (range 382345 843167))))