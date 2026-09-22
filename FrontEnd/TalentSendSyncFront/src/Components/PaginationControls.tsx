import { FaChevronLeft, FaChevronRight } from "react-icons/fa";

interface PaginationControlsProps {
  pageNumber: number;
  totalPages: number;
  totalCount: number;
  pageSize: number;
  onPageChange: (page: number) => void;
  label?: string;
}

export function PaginationControls({
  pageNumber,
  totalPages,
  totalCount,
  pageSize,
  onPageChange,
  label = "itens",
}: PaginationControlsProps) {
  const startItem = totalCount > 0 ? (pageNumber - 1) * pageSize + 1 : 0;
  const endItem = Math.min(pageNumber * pageSize, totalCount);

  return (
    <div className="flex items-center justify-between">
      <div className="text-sm text-slate-400">
        Mostrando {startItem} a {endItem} de {totalCount} {label}
      </div>
      <div className="flex items-center gap-2">
        <button
          onClick={() => onPageChange(Math.max(1, pageNumber - 1))}
          disabled={pageNumber <= 1}
          className="flex items-center gap-1 rounded-lg border border-slate-700 px-3 py-2 text-sm font-medium text-slate-300 transition-colors hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          <FaChevronLeft className="h-4 w-4" />
          Voltar
        </button>
        <span className="flex items-center px-3 text-sm text-slate-400">
          Página {pageNumber} de {Math.max(totalPages, 1)}
        </span>
        <button
          onClick={() => onPageChange(Math.min(totalPages, pageNumber + 1))}
          disabled={pageNumber >= totalPages || totalPages === 0}
          className="flex items-center gap-1 rounded-lg border border-slate-700 px-3 py-2 text-sm font-medium text-slate-300 transition-colors hover:bg-slate-800 disabled:opacity-50 disabled:cursor-not-allowed"
        >
          Avançar
          <FaChevronRight className="h-4 w-4" />
        </button>
      </div>
    </div>
  );
}