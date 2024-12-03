from setuptools import setup, find_packages

setup(
    name="sudoku-benchmark",
    version="1.0.0",
    description="Benchmarking Sudoku solvers using Python.",
    author="Mohammed Mosleh et Chaimae IMRANI",
    packages=find_packages(),
    install_requires=[
        "tensorflow==2.12.0",
        "numpy==1.23.5",
        "pandas==1.5.3",
        "benchmark==4.0.0"
    ],
    entry_points={
        "console_scripts": [
            "sudoku-benchmark=sudoku_benchmark:main",
        ],
    },
)
