use ToolStore


			Select p.ProductID, count(*) as als from Product p join
			(Select pr.ProductID, pr.PriceValue as PriceValue
			from Price pr join Statistic s on pr.StatisticID=s.StatisticID
			where s.CreationDate=(Select max(st.CreationDate) from Statistic st)
			and pr.PriceValue!=0
			intersect
			Select pr.ProductID, min(pr.PriceValue) as PriceValue
			from Price pr --join Statistic s on pr.StatisticID=s.StatisticID
			where pr.PriceValue!=0
			--where s.CreationDate=(Select max(st.CreationDate) from Statistic st)
			group by pr.ProductID) r on r.ProductID=p.ProductID
									join Vendor v on p.VendorID=v.VendorID

			group by p.ProductID
			order by als desc
			
			
			where p.VendorID=2			
			and p.Code='15895298'

			use ToolStore
			Select count(*) from Product
			where CurrentPrice!=0

	