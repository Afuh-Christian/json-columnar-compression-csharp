
# Json to Columnar Compression 


- Efficiently converts JSON to columnar format and back, enabling faster transmission and reduced storage. 
- This approach is especially beneficial when working with large datasets .
- You can provide JSON in any structure or format.


## Usage

```c#


// Not the corrent format in c# but you get the idea
var jsonArray = [
  {
    "id": 1,
    "name": "Alice",
    "age": 28,
    "city": "New York",
    "children": [
      { "id": 11, "name": "Anna", "age": 5 },
      { "id": 12, "name": "Alex", "age": 7 }
    ]
  },
  {
    "id": 2,
    "name": "Bob",
    "age": 34,
    "city": "Los Angeles",
    "children": [{ "id": 21, "name": "Ben", "age": 8 }]
  },
  { "id": 3, "name": "Charlie", "age": 25, "city": "Chicago", "children": null }
]



var compressed = JsonColumnarCompression.CompressJsonToColumnar(jsonArray)

// console.log(compressed)
// [
//   ["id", [1, 2, 3]],
//   ["name", ["Alice", "Bob", "Charlie"]],
//   ["age", [28, 34, 25]],
//   ["city", ["New York", "Los Angeles", "Chicago"]],
//   [
//     "children",
//     [
//       [
//         ["id", [11, 12]],
//         ["name", ["Anna", "Alex"]],
//         ["age", [5, 7]]
//       ],
//       [
//         ["id", [21]],
//         ["name", ["Ben"]],
//         ["age", [8]]
//       ],
//       null
//     ]
//   ]
// ]


var decompressed = JsonColumnarCompression.DecompressColumnarToJson(compressed)

// console.log(decompressed)
// [
//   {
//     "id": 1,
//     "name": "Alice",
//     "age": 28,
//     "city": "New York",
//     "children": [
//       { "id": 11, "name": "Anna", "age": 5 },
//       { "id": 12, "name": "Alex", "age": 7 }
//     ]
//   },
//   {
//     "id": 2,
//     "name": "Bob",
//     "age": 34,
//     "city": "Los Angeles",
//     "children": [{ "id": 21, "name": "Ben", "age": 8 }]
//   },
//   { "id": 3, "name": "Charlie", "age": 25, "city": "Chicago", "children": null }
// ]


```